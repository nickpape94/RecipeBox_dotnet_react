import React, { Fragment, useEffect, useState } from 'react';
import { Link, Redirect } from 'react-router-dom';
import PropTypes from 'prop-types';
import { connect } from 'react-redux';
import { getPosts } from '../../actions/post';
import { getUsers, getUser } from '../../actions/user';
import Spinner from '../layout/Spinner';
import PostItem from './PostItem';
import PageNavigation from './PageNavigation';
import pagination from '../../reducers/pagination';

const Posts = ({ getPosts, post: { posts, loading }, auth, pagination }) => {
	const [ pageNumber, setPageNumber ] = useState(1);
	useEffect(
		() => {
			getPosts(pageNumber);
		},
		[ getPosts, pageNumber ]
	);

	return loading ? (
		<Spinner />
	) : (
		<Fragment>
			<div className='post__navbar'>
				<div className='search__wrapper'>
					<input type='text' className='input' placeholder='Search for a recipe or cuisine...' />
					<div className='searchbtn'>
						<i className='fas fa-search' />
					</div>
				</div>
				<div className='post__dropdown'>
					<div className='dropdown'>
						<button className='dropbtn'>Sort Recipes By:</button>
						<div className='dropdown-content'>
							<Link to='!#'>Highest Rated</Link>
							<Link to='!#'>Oldest</Link>
							<Link to='!#'>Newest</Link>
							<Link to='!#'>Most Discussed</Link>
						</div>
					</div>
				</div>

				{/* <div className='post__dropdown'>
						<div class='dropdown'>
							<button class='dropbtn'>Sort Recipes By:</button>
							<div class='dropdown-content'>
								<Link to='!#'>Highest Rated</Link>
								<Link to='!#'>Oldest</Link>
								<Link to='!#'>Newest</Link>
								<Link to='!#'>Most Discussed</Link>
							</div>
						</div>
					</div> */}

				<div className='post__submit'>
					{auth.isAuthenticated && (
						<Link to='submit-post'>
							<div className='button'>Submit a Recipe</div>
						</Link>
					)}
					{!auth.isAuthenticated && (
						<Link to='login'>
							<div className='button'>Submit a Recipe</div>
						</Link>
					)}
				</div>
			</div>
			<PageNavigation pagination={pagination} pageNumber={pageNumber} setPageNumber={setPageNumber} />
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
	auth: PropTypes.object.isRequired,
	pagination: PropTypes.object.isRequired
};

const mapStateToProps = (state) => ({
	post: state.post,
	auth: state.auth,
	pagination: state.pagination
});

export default connect(mapStateToProps, { getPosts })(Posts);
