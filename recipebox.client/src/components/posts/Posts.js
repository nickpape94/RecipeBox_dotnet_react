import React, { Fragment, useEffect } from 'react';
import PropTypes from 'prop-types';
import { connect } from 'react-redux';
import { getPosts } from '../../actions/post';
import { getUsers, getUser } from '../../actions/user';
import Spinner from '../layout/Spinner';
import PostItem from './PostItem';

const Posts = ({ getPosts, post: { posts, loading } }) => {
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
			<div className='post'>{posts.map((post) => <PostItem key={post.postId} post={post} />)}</div>
		</Fragment>
	);
};

Posts.propTypes = {
	getPosts: PropTypes.func.isRequired,
	post: PropTypes.object.isRequired
};

console.log('hey');

const mapStateToProps = (state) => ({
	post: state.post
});

export default connect(mapStateToProps, { getPosts })(Posts);
