import React, { Fragment, useEffect } from 'react';
import { Link, Redirect } from 'react-router-dom';
import PropTypes from 'prop-types';
import { connect } from 'react-redux';
import { getPosts } from '../../actions/post';
import { getUsers, getUser } from '../../actions/user';
import Spinner from '../layout/Spinner';
import PostItem from './PostItem';

const Posts = ({ getPosts, post: { posts, loading }, auth }) => {
	useEffect(
		() => {
			getPosts();
		},
		[ getPosts ]
	);

	return loading ? (
		<Spinner />
	) : (
		<Fragment>
			<div className='post__navbar'>
				<div className='search__wrapper'>
					<input type='text' className='input' placeholder='Search for a recipe' />
					<div className='searchbtn'>
						<i class='fas fa-search' />
					</div>
				</div>
				{auth.isAuthenticated && (
					<Link to='!#'>
						<div className='button'>Submit a Recipe</div>
					</Link>
				)}
				{!auth.isAuthenticated && (
					<Link to='login'>
						<div className='button'>Submit a Recipe</div>
					</Link>
				)}
			</div>
			<div className='cards'>
				{posts.map((post) => (
					<PostItem
						key={post.postId}
						post={post}
						// postPhoto={post.postPhoto.filter((photo) => photo.isMain == true)}
					/>
				))}
			</div>
		</Fragment>
	);
};

Posts.propTypes = {
	getPosts: PropTypes.func.isRequired,
	post: PropTypes.object.isRequired,
	auth: PropTypes.object.isRequired
};

const mapStateToProps = (state) => ({
	post: state.post,
	auth: state.auth
});

export default connect(mapStateToProps, { getPosts })(Posts);
